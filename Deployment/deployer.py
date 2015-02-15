import os
import sys
import shutil
import elementtree.ElementTree as ET

this_file_path = os.path.dirname( os.path.realpath(__file__) )

class Deployer:
    def __init__( self, config_xml_file, deploy_xml_file ):
        self._live_object_root = ''
        self._namespace = ''
        self._deployment_dir = ''
        self._dll_deployment_dir = ''
        self._src_dll_path = ''
        self._additional_files = []
        
        self._parse_config_xml_file( config_xml_file )
        self._parse_deploy_xml_file( deploy_xml_file )
        
    def _parse_xml_file( self, xml_file ):
        tree = ET.parse(xml_file)
        return tree.getroot()
    
    def _parse_config_xml_file( self, config_xml_file ):
        root_xml_elt = self._parse_xml_file( config_xml_file )
        
        if root_xml_elt.find('LiveObjectRoot') != None:
            self._live_object_root = root_xml_elt.find('LiveObjectRoot').text.strip()
            
    def _parse_deploy_xml_file( self, deploy_xml_file ):
        root_xml_elt = self._parse_xml_file( deploy_xml_file )
        
        if root_xml_elt.find('namespace') != None:
            self._namespace = root_xml_elt.find('namespace').text.strip()
            namespace_id = self._namespace.split('`')[0]
            namespace_version = self._namespace.split('`')[1]
            self._deployment_dir = os.path.join(self._live_object_root, 'libraries', namespace_id, namespace_version)
            self._dll_deployment_dir = os.path.join(self._deployment_dir, 'data')
        
        self._src_dll_path = os.path.join( sys.argv[2] ).strip()
        
        if root_xml_elt.find('AdditionalFiles') != None:
            for additional_file in root_xml_elt.find('AdditionalFiles').findall('File'):
                additional_file_path = os.path.join(os.path.dirname(deploy_xml_file), additional_file.text)
                self._additional_files.append(additional_file_path)
        
    def _copy_file( self, src_file, dst_path ):
        shutil.copy( src_file, dst_path )
        
    def _create_directory( self, directory ):
        try:
            os.makedirs( directory )
        except:
            pass
        
    def _write_metadata_file( self ):
        metadata_file_path = os.path.join( self._deployment_dir, 'metadata.xml' )
        with open(metadata_file_path,'w') as metadata_file:
            line = '<Library id="' + self._namespace + '">' + '\n'
            line = line + '<Include filename="' + os.path.basename(self._src_dll_path) + '" />' + '\n'
            line = line + '</Library>'
            metadata_file.write(line)
    
    def _copy_additional_files( self ):
        for additional_file in self._additional_files:
            self._copy_file( additional_file, self._dll_deployment_dir )
            
    def deploy( self ):
        self._create_directory( self._dll_deployment_dir )
        self._copy_file( self._src_dll_path, self._dll_deployment_dir )
        self._write_metadata_file()
        self._copy_additional_files()
        

def main():
    config_xml_file = os.path.join( this_file_path, 'config.xml' )
    deploy_xml_file = os.path.join( sys.argv[1] ).strip()
    
    deployer = Deployer( config_xml_file, deploy_xml_file )
    deployer.deploy()
    

if __name__ == "__main__":
    main()
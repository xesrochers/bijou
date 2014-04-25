<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="text" indent="yes" />
		<xsl:param name="root" />
		<xsl:param name="index" />
		<xsl:param name="home" />

    <xsl:template match="root">
    	<ul>
  			<xsl:if test="$home='1'">
	    		<li>
	    			<a>
	    				<xsl:attribute name="href"><xsl:value-of select="$root" />/<xsl:value-of select="$index" /></xsl:attribute>
	    				<xsl:attribute name="class">icon-home</xsl:attribute>
	    				<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
	    			</a>
	    		</li>
	      </xsl:if>
      	<xsl:apply-templates select="node" />
      </ul>
    </xsl:template>

    <xsl:template match="node">
      	<xsl:for-each select=".">
      		<li>
      			<a>
      				<xsl:attribute name="href"><xsl:value-of select="$root" /><xsl:value-of select="@path" /><xsl:value-of select="$index" /></xsl:attribute>
      				<xsl:value-of select="@title"/>
      			</a>
      			<xsl:if test="node">
				    	<ul>
	      				<xsl:apply-templates select="node" />
				      </ul>
				     </xsl:if>
      		</li>
    		</xsl:for-each>
    </xsl:template>

</xsl:stylesheet>
